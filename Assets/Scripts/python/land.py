import airsim
client: airsim.MultirotorClient = airsim.MultirotorClient()
client.confirmConnection()
client.enableApiControl(True)
client.landAsync()
