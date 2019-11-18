using System;
using System.Diagnostics;
using System.Linq;

namespace devshop_server {
    public interface IState {
        int InboxStoryCount { get; }
        int BacklogStoryCount { get; }
        int DevStoryCount { get; }
        int TestStoryCount { get; }
        int DoneStoryCount { get; }
        bool FounderFree { get; }
        int DevsFree { get; }
        int TestersFree { get; }
        int BasFree { get; }
        int DevCount { get; }
        int TesterCount { get; }
        int BaCount { get; }
        int Bank { get; }
        int NewProjectCost { get; }
        int DevHireCost { get; }
        int TestHireCost { get; }
        int BaHireCost { get; }
        int DevUpgradeCost { get; }
        int TestUpgradeCost { get; }
        int BaUpgradeCost { get; }
        int DevMinLevel { get; }
        int TestMinLevel { get; }
        int BaMinLevel { get; }
        bool CanHireDev { get; }
        bool CanHireTest { get; }
        bool CanHireBa { get; }
        bool CanUpgradeDev { get; }
        bool CanUpgradeTest { get; }
        bool CanUpgradeBa { get; }

        void UpdateState();
        void UpdateWorkerHireCost();
        void UpdateWorkerInformation();
        void UpdateWorkerUpgradeCost();
    }

    public class State : IState {
        private static bool _visitedStore = false;
        private static bool _devUpgradeEnabled = false;
        private static bool _testUpgradeEnabled = false;
        private static bool _baUpgradeEnabled = false;
        public int InboxStoryCount { get; private set; }
        public int BacklogStoryCount { get; private set; }
        public int DevStoryCount { get; private set; }
        public int TestStoryCount { get; private set; }
        public int DoneStoryCount { get; private set; }
        public bool FounderFree { get; private set; }
        public int DevsFree { get; private set; }
        public int TestersFree { get; private set; }
        public int BasFree { get; private set; }
        public int DevCount { get; private set; }
        public int TesterCount { get; private set; }
        public int BaCount { get; private set; }
        public int Bank { get; private set; }
        public int NewProjectCost { get; private set; }
        public int DevHireCost { get; private set; }
        public int TestHireCost { get; private set; }
        public int BaHireCost { get; private set; }
        public int DevUpgradeCost { get; private set; }
        public int TestUpgradeCost { get; private set; }
        public int BaUpgradeCost { get; private set; }

        public int DevMinLevel { get; private set; }
        public int TestMinLevel { get; private set; }
        public int BaMinLevel { get; private set; }

        public bool CanHireDev { get; private set; }
        public bool CanHireTest { get; private set; }
        public bool CanHireBa { get; private set; }

        public bool CanUpgradeDev { get; private set; }
        public bool CanUpgradeTest { get; private set; }
        public bool CanUpgradeBa { get; private set; }

        private WorkerPool _workers;
        private KanbanBoard _board;
        private Store _store;
        private static int loopCount = 0;
        public State(WorkerPool workers, KanbanBoard board, Store store) {
            _workers = workers;
            _board = board;
            _store = store;
        }

        public void UpdateState() {
            var start = DateTime.Now;
            InboxStoryCount = _board.InboxStoryCount;
            BacklogStoryCount = _board.BacklogStoryCount;
            DevStoryCount = _board.DevStoryCount;
            TestStoryCount = _board.TestStoryCount;
            DoneStoryCount = _board.DoneStoryCount;
            Debug.WriteLine($"Story count in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;

            FounderFree = _workers.Workers.Where(w => w.Id == "p1").FirstOrDefault()?.isFree ?? false;
            Debug.WriteLine($"Founder Free in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;

            DevsFree = _workers.IdleWorkerCount(WorkerTypes.dev);
            TestersFree = _workers.IdleWorkerCount(WorkerTypes.test);
            BasFree = _workers.IdleWorkerCount(WorkerTypes.ba);
            Debug.WriteLine($"idle count in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;

            Bank = _store.TotalMoneyAvailable;
            NewProjectCost = _store.NewProjectCost;
            Debug.WriteLine($"Bank and Project Cost in: {DateTime.Now.Subtract(start).TotalMilliseconds})");

            if (DevUpgradeCost == int.MaxValue || TestUpgradeCost == int.MaxValue || BaUpgradeCost == int.MaxValue) {
                UpdateWorkerUpgradeCost();
            }

            if (DevHireCost == int.MaxValue || TestHireCost == int.MaxValue || BaHireCost == int.MaxValue) {
                UpdateWorkerHireCost();
            }

            CanHireDev = DevHireCost < Bank;
            CanHireTest = TestHireCost < Bank;
            CanHireBa = BaHireCost < Bank;

            CanUpgradeDev = _devUpgradeEnabled && DevUpgradeCost < Bank;
            CanUpgradeTest = _testUpgradeEnabled && TestUpgradeCost < Bank;
            CanUpgradeBa = _baUpgradeEnabled && BaUpgradeCost < Bank;

            if (!_devUpgradeEnabled) {
                _devUpgradeEnabled = _store.IsStoreItemAvailable(WorkerTypes.dev);
            }
            if (!_testUpgradeEnabled) {
                _testUpgradeEnabled = _store.IsStoreItemAvailable(WorkerTypes.test);
            }
            if (!_baUpgradeEnabled) {
                _baUpgradeEnabled = _store.IsStoreItemAvailable(WorkerTypes.ba);
            }
            loopCount++;
        }

        public void UpdateWorkerUpgradeCost() {
            if (!_visitedStore && _store.StoreAvailable) {
                _store.GoToStore();
                _store.GoToKanban();
            }
            DevUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.dev);
            TestUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.test);
            BaUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.ba);
        }

        public void UpdateWorkerInformation() {
            _workers.UpdateWorkers();

            DevCount = _workers.Workers.Where(w => w.Type == WorkerTypes.dev).Count();
            TesterCount = _workers.Workers.Where(w => w.Type == WorkerTypes.test).Count();
            BaCount = _workers.Workers.Where(w => w.Type == WorkerTypes.ba).Count();

            DevMinLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.dev);
            TestMinLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.test);
            BaMinLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.ba);

            UpdateWorkerHireCost();
            UpdateWorkerUpgradeCost();
        }

        public void UpdateWorkerHireCost() {
            DevHireCost = _store.WorkerHireCost(WorkerTypes.dev);
            TestHireCost = _store.WorkerHireCost(WorkerTypes.test);
            BaHireCost = _store.WorkerHireCost(WorkerTypes.ba);
        }
    }
}