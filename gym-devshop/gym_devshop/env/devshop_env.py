import random
import json
import gym
import time
from gym import spaces
import pandas as pd
import numpy as np
from client import DevshopClient

import logging
logger = logging.getLogger(__name__)

MAX_BANK = 2147483647
MAX_NUM_SHARES = 2147483647
MAX_SHARE_PRICE = 5000
MAX_OPEN_POSITIONS = 5
MAX_STEPS = 2000

INITIAL_ACCOUNT_BALANCE = 10000
N_DISCRETE_ACTIONS = 5


class DevshopEnv(gym.Env):
    metadata = {'render.modes': ['human']}

    def __init__(self):

        self.reward_range = (-100, 100)
        # Define action and observation space
        # They must be gym.spaces objects
        # Example when using discrete actions:
        # 0 - Add Project, 1 - FounderDoBaWork, 2 - FounderDoDevWork, 3 - FounderDoTestWork
        self.action_space = spaces.Discrete(N_DISCRETE_ACTIONS)
        # Example for using image as input:
        # self.observation_space = spaces.Box(low=0, high=255, shape=
        #               (HEIGHT, WIDTH, N_CHANNELS), dtype=np.uint8)

        self.observation_space = spaces.Box(
            low=-1, high=1, shape=(7,), dtype=np.float32)

        self.lastBanks = []
        self.lastInboxStoryCount = 0

    def _take_action(self, action):
        # print(f'Step: {self.current_step}')

        # call the api to get the action to occur
        client = DevshopClient()
        response = client.doAction(action)
        return response == b'true'

    def step(self, action):
        # Execute one time step within the environment
        self.actionReward = 0
        self.current_step += 1
        delay_modifier = (self.current_step / MAX_STEPS)
        self.didAction = False
        client = DevshopClient()
        state = client.getState()

        if action == 0:
            self.founderWasBusy = not state.founderFree
            time.sleep(2)
            self.didAction = True
        else:
            self.didAction = self._take_action(action)

        state = client.getState()

        self.bank = state.bank
        self.inboxStoryCount = state.inboxStoryCount
        self.backlogStoryCount = state.backlogStoryCount
        self.devStoryCount = state.devStoryCount
        self.testStoryCount = state.testStoryCount
        self.doneStoryCount = state.doneStoryCount
        self.founderFree = state.founderFree
        self.newProjectCost = state.newProjectCost

        if self.didAction:
            if action == 0:
                if self.founderWasBusy:
                    self.actionReward = 0
                else:
                    self.actionReward = -1 - 99 * delay_modifier
            if action == 1:
                if state.inboxStoryCount < 3:
                    self.actionReward = 5
                else:
                    tooMany = -10 -10*state.inboxStoryCount* delay_modifier
                    if tooMany < -100:
                        tooMany = -100
                    self.actionReward = tooMany
            if action == 2:
                self.actionReward = 10
            if action == 3:
                self.actionReward = 30
            if action == 4:
                self.actionReward = 35 + 70 * delay_modifier
        else:
            self.actionReward = 0
        # self.lastBanks.append(self.bank)
        reward = self.actionReward
        done = self.bank + 2000 <= 0

        obs = np.array((self.inboxStoryCount/100,
                        self.backlogStoryCount/100,
                        self.testStoryCount/100,
                        self.doneStoryCount / 100,
                        self.founderFree,
                        self.newProjectCost/150,
                        self.bank / 2000))

        print(
            f"step: {self.current_step}; Done Stories: {self.doneStoryCount}; Reward: {reward} for action: {action}")
        return obs, reward, done, {}

    def reset(self):
        self.current_step = 0
        self.lastInboxStoryCount = 0

        client = DevshopClient()
        client.doReset()

    def render(self, mode='human', close=False):
        # Render the environment to the screen
        print(f'Step: {self.current_step}')
        print(f'bank: {self.bank}')
        print(
            f'Story Counts: {self.inboxStoryCount}|{self.backlogStoryCount}|{self.devStoryCount}|{self.testStoryCount}')
        print(self.founderFree)
        print(f'New project cost: {self.newProjectCost}')
