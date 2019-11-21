import gym
import json
import datetime as dt
from stable_baselines.common.policies import MlpPolicy
from stable_baselines.common.vec_env import DummyVecEnv
from stable_baselines import PPO2
from env.devshop_env import DevshopEnv

# The algorithms require a vectorized environment to run
env = DummyVecEnv([lambda: DevshopEnv()])
model = PPO2(MlpPolicy, env, verbose=1)
model.learn(total_timesteps=20000)
obs = env.reset()
for i in range(20000):
  action, _states = model.predict(obs)
  obs, rewards, done, info = env.step(action)
  env.render()