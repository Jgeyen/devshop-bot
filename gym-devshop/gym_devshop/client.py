import json
import requests
from collections import namedtuple

api_url_base = 'https://localhost:5001/api/'

URL_STATE = "state"
MAX_NUM_SHARES = 2147483647
MAX_SHARE_PRICE = 5000
MAX_OPEN_POSITIONS = 5
MAX_STEPS = 20000


class DevshopClient():

    # InboxStoryCount - simple 0-100
    # BacklogStoryCount - simple 0-100
    # DevStoryCount - simple 0-100
    # TestStoryCount - simple 0-100
    # DoneStoryCount - simple 0-100
    # FounderFree - simple 0-1
    # DevsFree
    # TestersFree
    # BasFree
    # DevCount
    # TesterCount
    # BaCount
    # Bank - simple 0-20000
    # NewProjectCost - simple 0-1000
    # DevHireCost
    # TestHireCost
    # BaHireCost
    # DevUpgradeCost
    # TestUpgradeCost
    # BaUpgradeCost
    # DevMinLevel
    # TestMinLevel
    # BaMinLevel
    # CanHireDev
    # CanHireTest
    # CanHireBa
    # CanUpgradeDev
    # CanUpgradeTest
    # CanUpgradeBa
    def getState(self):
        requestUrl = api_url_base + URL_STATE
        response = requests.get(requestUrl, verify=False)

        x = json.loads(response.content, object_hook=lambda d: namedtuple('X', d.keys())(*d.values()))

        # print(x.bank)
        # print(myobj)
        return x

    def doReset(self):
        requestUrl = api_url_base + "game/reset"
        response = requests.get(requestUrl, verify=False)


    def doAction(self, action):
        actions = ["AddProject",
                   "FounderDoBaWork",
                   "FounderDoDevWork",
                   "FounderDoTestWork",
                   "DoBaWork",
                   "DoDevWork",
                   "DoTestWork",
                   "HireBa",
                   "HireDev",
                   "HireTester",
                   "UpgradeBa",
                   "UpgradeDev",
                   "UpgradeTester"]
        actionToPerform = api_url_base + "actions/"+actions[action]

        response = requests.post(actionToPerform, json="{}", verify=False)
        print("Response: {} For action: {}".format(
            response.status_code, actions[action]))
        return response.content