using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

namespace devshop_server {
    public enum WorkerTypes {
        ba,
        dev,
        test,
        founder
    }

    public interface IWorkerPool {
        List<Worker> Workers { get; }
        Worker GetIdleWorker(WorkerTypes type);
        int GetLowestSkillLevelForWorker(WorkerTypes type);
        Worker GetLowestSkillWorker(WorkerTypes type);
        int IdleWorkerCount(WorkerTypes type);
        void UpdateWorkers();
    }

    public class WorkerPool : IWorkerPool {
        public List<Worker> Workers { get; private set; }
        private IDriver _driver;

        public WorkerPool(IDriver driver) {
            _driver = driver;
            Workers = new List<Worker>();
        }

        public void UpdateWorkers() {
            Workers = GetWorkers();
        }
        private List<Worker> GetWorkers() {
            var workers = new List<Worker>();
            var workerIds = _driver.GetIds(By.CssSelector($".person.doer"));
            foreach (var workerId in workerIds) {
                workers.Add(new Worker(workerId, _driver));
            }
            return workers;
        }

        public int IdleWorkerCount(WorkerTypes type) {
            return _driver.CDriver.FindElementsByCssSelector($".person.doer.{type}:not(.busy):not(#p1)").Count();
        }
        public Worker GetIdleWorker(WorkerTypes type) {
            string workerId;
            if (type == WorkerTypes.founder) {
                workerId = _driver.GetIds(By.CssSelector($"#p1:not(.busy)")).FirstOrDefault();
            } else {
                workerId = _driver.GetIds(By.CssSelector($"#people .person.doer.{type}:not(.busy)")).FirstOrDefault();
            }
            return workerId != null ? new Worker(workerId, _driver) : null;
        }
        public Worker GetLowestSkillWorker(WorkerTypes type) {
            var lowestSkill = _driver.GetLowestSkillForWorkers(type);
            return lowestSkill == null ? null :
                    Workers.Where(w => w.Type == type && w.SkillLevel == lowestSkill).OrderBy(w => w.isFree).FirstOrDefault();
        }

        public int GetLowestSkillLevelForWorker(WorkerTypes type) {
            int? skillLevel = 1;
            for (var i = 0; i < 3; i++) {
                try {
                    var skillLevels = _driver.CDriver.FindElementsByCssSelector($".person:not(#p1) .skill.{type}")?.Select(w => int.Parse(w.GetAttribute("data-level")));
                    skillLevel = skillLevels?.OrderBy(s => s).FirstOrDefault();
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return skillLevel ?? 1;
        }
    }

    public class Worker {
        private IDriver _driver = null;
        private List<Skill> _skillz;
        public string Id;
        public int SkillLevel => _skillz.FirstOrDefault(s => s.Type == (Skill.SkillType)Type)?.Level ?? 0;
        public WorkerTypes Type => _skillz.Count > 1 ? WorkerTypes.founder : (WorkerTypes)_skillz.First().Type;

        public bool isFree => !_driver.GetElementAttributeText(By.Id(Id), "class").Contains("busy");

        public Worker(string id, IDriver driver) {
            _driver = driver;
            Id = id;

            _skillz = new List<Skill>();
            var skills = _driver.GetSkillsForWorker(id);

            foreach (var skill in skills) {
                if (skill.skillClass.Contains("dev")) _skillz.Add(new Skill(Skill.SkillType.dev, skill.level));
                if (skill.skillClass.Contains("test")) _skillz.Add(new Skill(Skill.SkillType.test, skill.level));
                if (skill.skillClass.Contains("ba")) _skillz.Add(new Skill(Skill.SkillType.ba, skill.level));
            }
        }

        public void Select() {
            _driver.ClickItem(By.Id(Id));
        }
        public void SelectWithWait(TimeSpan timeout) {
            var endTime = DateTime.Now + timeout;
            while (!isFree && endTime > DateTime.Now) {
                Thread.Sleep(10);
            }
            Select();
        }
    }
    public class Skill {
        public enum SkillType {
            ba,
            dev,
            test
        }
        public SkillType Type { get; private set; }
        public int Level { get; private set; }

        public Skill(SkillType type, int level) {
            Type = type;
            Level = level;
        }
    }
}