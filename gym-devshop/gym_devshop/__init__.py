from gym.envs.registration import register

register(
    id='devshop-v0',
    entry_point='gym_devshop.envs:DevshopEnv',
)
register(
    id='devshop-hard-v0',
    entry_point='gym_devshop.envs:DevshopHardEnv',
)