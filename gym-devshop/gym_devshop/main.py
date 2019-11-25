import gym
import json
import datetime as dt
from stable_baselines.deepq.policies import MlpPolicy
from stable_baselines.common.vec_env import DummyVecEnv
from stable_baselines import DQN
from env.devshop_env import DevshopEnv

import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
# The algorithms require a vectorized environment to run

env = DummyVecEnv([lambda: DevshopEnv()])
# env = gym.make(DevshopEnv)
model = DQN(MlpPolicy, env, verbose=1)
model.learn(total_timesteps=2000)
obs = env.reset()
for i in range(2000):
  env.render()
  action, _states = model.predict(obs)
  obs, rewards, done, info = env.step(action)