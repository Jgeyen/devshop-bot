import random
import json
import gym
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
MAX_STEPS = 20000

INITIAL_ACCOUNT_BALANCE = 10000
N_DISCRETE_ACTIONS = 4


class DevshopEnv(gym.Env):
    metadata = {'render.modes': ['human']}

    def __init__(self):

        self.reward_range = (-200, MAX_BANK)
        # Define action and observation space
        # They must be gym.spaces objects
        # Example when using discrete actions:
        # 0 - Add Project, 1 - FounderDoBaWork, 2 - FounderDoDevWork, 3 - FounderDoTestWork
        self.action_space = spaces.Discrete(N_DISCRETE_ACTIONS)
        # Example for using image as input:
        # self.observation_space = spaces.Box(low=0, high=255, shape=
        #               (HEIGHT, WIDTH, N_CHANNELS), dtype=np.uint8)

        self.observation_space = self.observation_space = spaces.Tuple((
                                    spaces.Discrete(101),
                                    spaces.Discrete(101),
                                    spaces.Discrete(101),
                                    spaces.Discrete(101),
                                    spaces.Discrete(101),
                                    spaces.Discrete(2),
                                    spaces.Discrete(1001)))

    def _take_action(self, action):
        print(f'Step: {self.current_step}')

        #call the api to get the action to occur
        client = DevshopClient()
        client.doAction(action)

    def step(self, action):
        # Execute one time step within the environment
        self._take_action(action)

        self.current_step += 1

        delay_modifier = (self.current_step / MAX_STEPS)

        client = DevshopClient()
        state = client.getState()
        self.bank = state.bank
        self.inboxStoryCount = state.InboxStoryCount
        self.backlogStoryCount = state.BacklogStoryCount
        self.devStoryCount = state.DevStoryCount
        self.testStoryCount = state.TestStoryCount
        self.doneStoryCount = state.DoneStoryCount
        self.founderFree = state.FounderFree
        self.newProjectCost = state.NewProjectCost

        reward = self.bank * delay_modifier
        done = self.bank + 1000 <= 0 #discreet space can't handle negative, 1000 is going to be our y intercept

        

        obs = (self.inboxStoryCount, 
                self.backlogStoryCount,
                self.devStoryCount,
                self.testStoryCount,
                self.doneStoryCount,
                self.founderFree,
                self.newProjectCost)

        return obs, reward, done, {}

    def reset(self):
        self.current_step = 0
        client = DevshopClient()
        client.doReset()

    def render(self, mode='human', close=False):
        # Render the environment to the screen
        print(f'Step: {self.current_step}')
        print(f'bank: {self.bank}')
        print(f'Story Counts: {self.inboxStoryCount}|{self.backlogStoryCount}|{self.devStoryCount}|{self.testStoryCount}|{self.doneStoryCount}')
        print(self.founderFree)
        print(f'New project cost: {self.newProjectCost}')