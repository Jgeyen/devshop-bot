import random
import json
import gym
from gym import spaces
import pandas as pd
import numpy as np

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

        self.observation_space = spaces.Box(
            0, 1, shape=(8, 8), dtype=np.float32)

    def _take_action(self, action):
         print(f'Step: {self.current_step}')
        #action_type = action[0]

        #call the api to get the action to occur
        
        # if action_type < 1:
            # Buy amount % of balance in shares
            # total_possible = int(self.balance / current_price)
            # shares_bought = int(total_possible * amount)
            # prev_cost = self.cost_basis * self.shares_held
            # additional_cost = shares_bought * current_price

            # self.balance -= additional_cost
            # self.cost_basis = (
            #     prev_cost + additional_cost) / (self.shares_held + shares_bought)
            # self.shares_held += shares_bought

        # elif action_type < 2:
            # Sell amount % of shares held
        #     shares_sold = int(self.shares_held * amount)
        #     self.balance += shares_sold * current_price
        #     self.shares_held -= shares_sold
        #     self.total_shares_sold += shares_sold
        #     self.total_sales_value += shares_sold * current_price

        # self.net_worth = self.balance + self.shares_held * current_price

        # if self.net_worth > self.max_net_worth:
        #     self.max_net_worth = self.net_worth

        # if self.shares_held == 0:
        #     self.cost_basis = 0

    def _next_observation(self):
        return 1
    def step(self, action):
        # Execute one time step within the environment
        self._take_action(action)

        self.current_step += 1

        delay_modifier = (self.current_step / MAX_STEPS)

        reward = self.balance * delay_modifier
        done = self.net_worth <= 0

        obs = self._next_observation()

        return obs, reward, done, {}

    def reset(self):
        # Reset the state of the environment to an initial state
        # Reset the state of the environment to an initial state
        self.balance = INITIAL_ACCOUNT_BALANCE
        self.net_worth = INITIAL_ACCOUNT_BALANCE
        self.max_net_worth = INITIAL_ACCOUNT_BALANCE
        self.shares_held = 0
        self.cost_basis = 0
        self.total_shares_sold = 0
        self.total_sales_value = 0

        # Set the current step to a random point within the data frame
        self.current_step = 0

        return self._next_observation()

    def render(self, mode='human', close=False):
        # Render the environment to the screen
        # Render the environment to the screen
        profit = self.net_worth - INITIAL_ACCOUNT_BALANCE

        print(f'Step: {self.current_step}')
        print(f'Balance: {self.balance}')
        print(
            f'Shares held: {self.shares_held} (Total sold: {self.total_shares_sold})')
        print(
            f'Avg cost for held shares: {self.cost_basis} (Total sales value: {self.total_sales_value})')
        print(
            f'Net worth: {self.net_worth} (Max net worth: {self.max_net_worth})')
        print(f'Profit: {profit}')