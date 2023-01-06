# import sys

import airsim
# import cv2

# region parameters

camera_id = 0
# params = sys.argv
filename = "captured_image.png"
# if len(params) > 1:
#     camera_id = int(params[0])
#     filename = params[1]

# endregion

client: airsim.MultirotorClient = airsim.MultirotorClient()
client.confirmConnection()
client.enableApiControl(True)
image = client.simGetImage(str(camera_id), airsim.ImageType.Scene)
# img_array = cv2.imdecode(airsim.string_to_uint8_array(image), cv2.IMREAD_UNCHANGED)
# cv2.imwrite(filename, cv2.cvtColor(img_array, cv2.COLOR_BGR2RGB))
# cv2.imwrite(filename, img_array)