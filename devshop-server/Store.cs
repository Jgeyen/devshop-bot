using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace devshop_server {
    public enum StoreItems {
        UpskillTester = 50,
        UpskillDeveloper = 20,
        UpskillBA = 107,
        MechanicalKeyboard = 12
    }

    public interface IStore {
        bool StoreAvailable { get; }
        int TotalMoneyAvailable { get; }
        int NewProjectCost { get; }

        void GoToKanban();
        void GoToStore();
        void HireWorker(WorkerTypes type);
        bool IsStoreItemAvailable(WorkerTypes type);
        void PurchaseStoreItem(StoreItems item);
        int WorkerHireCost(WorkerTypes type);
        int WorkerUpdateCost(WorkerTypes type);
    }

    public class Store : IStore {
        private static IDriver _driver;

        public Store(IDriver driver) {
            _driver = driver;
        }

        public void PurchaseStoreItem(StoreItems item) {
            new StoreItem(_driver, item).Purchase();
        }

        public bool StoreAvailable => _driver.CDriver.FindElementsByCssSelector(".button.visitStore.hint").Any();
        public void GoToStore() {
            _driver.ClickItem(By.CssSelector(".button.visitStore"));
        }
        public void GoToKanban() {
            _driver.ClickItem(By.Id("closeStore"));
        }
        public int TotalMoneyAvailable => ExtractMoney(_driver.GetElementText(By.Id("money"))) ?? int.MinValue;
        public int NewProjectCost => ExtractMoney(_driver.GetElementText(By.Id("getLead"))) ?? int.MinValue;

        public int WorkerHireCost(WorkerTypes type) {
            return ExtractMoney(_driver.GetElementText(By.CssSelector($"div.getPerson.{type}:not(.hidden)"))) ?? int.MaxValue;
        }

        public void HireWorker(WorkerTypes type) {
            _driver.ClickItem(By.CssSelector(($"div.getPerson.{type}:not(.hidden)")));
        }
        public bool IsStoreItemAvailable(WorkerTypes type) {
            var item = type switch
            {
                WorkerTypes.dev => StoreItems.UpskillDeveloper,
                WorkerTypes.test => StoreItems.UpskillTester,
                WorkerTypes.ba => StoreItems.UpskillBA
            };
            return _driver.CDriver.FindElementsByCssSelector($".storeItem-catalog.item-enabled #store-button-{(int)item}").Any();
        }
        public int WorkerUpdateCost(WorkerTypes type) {
            var item = type switch
            {
                WorkerTypes.dev => StoreItems.UpskillDeveloper,
                WorkerTypes.test => StoreItems.UpskillTester,
                WorkerTypes.ba => StoreItems.UpskillBA
            };
            return ExtractMoney(_driver.GetElementAttributeText(By.CssSelector($"#store-button-{(int)item}"), "innerText")) ?? int.MaxValue;
        }
        public static int? ExtractMoney(string text) {
            var moneyText = Regex.Match(text, @"-?\d+").Value;
            return int.TryParse(moneyText, out int result) ? (int?)result : null;
        }
    }
    public class StoreItem {
        private string _id;
        private IDriver _driver;
        public StoreItem(IDriver driver, StoreItems item) {
            _driver = driver;
            _id = $"store-button-{(int)item}";
        }
        public int Cost => Store.ExtractMoney(_driver.GetElementText(By.CssSelector($"#items>.storeItem-catalog.item-enabled>#{_id}"))) ?? int.MaxValue;
        public void Purchase() {
            _driver.ClickItem(By.Id(_id));
        }
    }
}