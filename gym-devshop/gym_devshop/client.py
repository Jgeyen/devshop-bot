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

    def getState(self):
        requestUrl = api_url_base + URL_STATE
        response = requests.get(requestUrl, verify=False)
        print(response.json())
        myobj = response.json()

        x = json.loads(response.content, object_hook=lambda d: namedtuple(
        'X', d.keys())(*d.values()))

        print(x.bank)
        print(myobj)

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

        response = requests.post(actionToPerform, json= "{}", verify=False)
        print("Response: {} For action: {}".format(response.status_code, actions[action]))

d = DevshopClient()
d.doAction(action=0)