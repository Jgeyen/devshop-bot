using System;

namespace devshop_server {
    public interface IActions {
        void AddProject();
        bool DoBaWork();
        bool DoDevWork();
        bool DoTestWork();
        bool FounderDoBaWork();
        bool FounderDoDevWork();
        bool FounderDoTestWork();
        void HireBa();
        void HireDev();
        void HireTester();
        void UpgradeBa();
        void UpgradeDev();
        void UpgradeTester();
    }

    public class Actions : IActions {
        private IWorkerPool _workers;
        private IKanbanBoard _board;
        private IStore _store;
        private IState _state;
        public Actions(IWorkerPool workers, IKanbanBoard board, IStore store, IState state) {
            _workers = workers;
            _board = board;
            _store = store;
            _state = state;
        }

        public bool FounderDoDevWork() {
            return WorkerDoWork(WorkerTypes.founder, StoryTypes.dev);
        }
        public bool FounderDoTestWork() {
            var outcome = WorkerDoWork(WorkerTypes.founder, StoryTypes.test);
            if(outcome) _board.IncrementDoneCount();
            return outcome;
        }
        public bool FounderDoBaWork() {
            return WorkerDoWork(WorkerTypes.founder, StoryTypes.ba);
        }
        public bool DoDevWork() {
            return WorkerDoWork(WorkerTypes.dev, StoryTypes.dev);
        }
        public bool DoTestWork() {
            return WorkerDoWork(WorkerTypes.test, StoryTypes.test);
        }
        public bool DoBaWork() {
            return WorkerDoWork(WorkerTypes.ba, StoryTypes.ba);
        }
        private bool WorkerDoWork(WorkerTypes workerType, StoryTypes storyType) {
            var foundWork = false;
            var worker = _workers.GetIdleWorker(workerType);
            var work = _board.FindNextWork(storyType);
            if (worker != null && work != null) {
                worker.Select();
                work.Select();
                foundWork = true;
            }
            return foundWork;
        }
        public void HireDev() {
            if (!_state.CanHireDev) return;
            HireWorker(WorkerTypes.dev);
        }
        public void HireTester() {
            if (!_state.CanHireTest) return;
            HireWorker(WorkerTypes.test);
        }
        public void HireBa() {
            if (!_state.CanHireBa) return;
            HireWorker(WorkerTypes.ba);
        }
        private void HireWorker(WorkerTypes type) {
            _store.HireWorker(type);
            _state.UpdateWorkerInformation();
        }
        public void UpgradeDev() {
            if (!_state.CanUpgradeDev) return;

            UpgradeWorker(WorkerTypes.dev);
        }
        public void UpgradeTester() {
            if (!_state.CanUpgradeTest) return;

            UpgradeWorker(WorkerTypes.test);
        }
        public void UpgradeBa() {
            if (!_state.CanUpgradeBa) return;

            UpgradeWorker(WorkerTypes.ba);
        }
        private void UpgradeWorker(WorkerTypes type) {
            _store.GoToStore();
            var storeItemType = type switch
            {
                WorkerTypes.dev => StoreItems.UpskillDeveloper,
                WorkerTypes.test => StoreItems.UpskillTester,
                WorkerTypes.ba => StoreItems.UpskillBA,
                WorkerTypes.founder => StoreItems.UpskillDeveloper
            };

            _store.PurchaseStoreItem(storeItemType);
            _store.GoToKanban();
            _board.SelectPurchasedStoreItem(type);
            _workers.GetLowestSkillWorker(type)?.SelectWithWait(TimeSpan.FromSeconds(10));
            _state.UpdateWorkerInformation();

        }
        public void AddProject() {
            _board.AddProject();
        }
    }
}