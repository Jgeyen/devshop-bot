using System.Linq;
using OpenQA.Selenium;

namespace devshop_server {
    public enum StoryTypes {
        ba,
        dev,
        test
    }

    public interface IKanbanBoard {
        int InboxStoryCount { get; }
        int BacklogStoryCount { get; }
        int DevStoryCount { get; }
        int TestStoryCount { get; }
        int DoneStoryCount { get; }
        int IncrementDoneCount();
        void ResetBoard();
        void AddProject();
        Story FindNextWork(StoryTypes type);
        int? GetAttributeTextAsInt(By by, string attribute);
        void SelectPurchasedStoreItem(WorkerTypes type);
    }

    public class KanbanBoard : IKanbanBoard {
        public int InboxStoryCount => GetAttributeTextAsInt(By.CssSelector("#ba .count"), "data-count") ?? 0;
        public int BacklogStoryCount => GetAttributeTextAsInt(By.CssSelector("#dev .count"), "data-count") ?? 0;
        public int DevStoryCount => GetAttributeTextAsInt(By.CssSelector("#dev0 .count"), "data-count") ?? 0;
        public int TestStoryCount => GetAttributeTextAsInt(By.CssSelector("#test .count"), "data-count") ?? 0;
        public int DoneStoryCount => doneCount;
        private IDriver _driver;
        private static int doneCount = 0;
        public KanbanBoard(IDriver driver) {
            _driver = driver;
        }

        public int? GetAttributeTextAsInt(By by, string attribute) {
            return int.TryParse(_driver.GetElementAttributeText(by, attribute), out int response) ? (int?)response : null;
        }
        public Story FindNextWork(StoryTypes type) {
            var story = _driver.CDriver.FindElementsByCssSelector($"span.story.{type}:not(.busy)").FirstOrDefault()?.GetAttribute("id") ?? "";
            return story != "" ? new Story(story, _driver) : null;
        }

        public void AddProject() {
            _driver.ClickItem(By.Id("getLead"));
        }

        public void SelectPurchasedStoreItem(WorkerTypes type) {
            _driver.ClickItem(By.CssSelector($".storeItem.receiver.{type}"));
        }
        public int IncrementDoneCount() {
            doneCount++;
            return doneCount;
        }

        public void ResetBoard() {
            doneCount=0;
        }
    }

    public class Story {
        private string _id;
        private IDriver _driver;
        public Story(string id, IDriver driver) {
            _id = id;
            _driver = driver;
        }
        public void Select() {
            _driver.ClickItem(By.Id(_id));
        }
    }
}
