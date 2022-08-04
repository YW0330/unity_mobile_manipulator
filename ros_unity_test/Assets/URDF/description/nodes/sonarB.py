#!/usr/bin/env python

import rospy
import roslib
import math
from math import pi as PI, degrees, radians, sin, cos
from std_msgs.msg import Int16,Int32
from std_msgs.msg import String
from sensor_msgs.msg import Range
import sensor_msgs.point_cloud2 as pc2
from sensor_msgs.msg import PointCloud2, PointField 



def B_callback(sonar_data):
    
    sonar_rB = 0.0
    
    safe_range_0 = 10
    safe_range_1 = 7
    
    sonar_pub_cloud = rospy.Publisher("/sonar_cloudpoint_B", PointCloud2, queue_size=5)
    
    sonar_height = 0
    sonar_maxval = 3.5
    
    sonarB_offset_yaw = 3.1416
    sonarB_offset_x = -0.20
    sonarB_offset_y = 0.0

    sonar_cloud = [[100.0,0.105,0.1],[100.0,-0.105,0.1],[0.2,100.0,0.1],[0.2,-100.0,0.1]]

    sonar_cloud[3][0] = sonarB_offset_x + sonar_maxval * math.cos(sonarB_offset_yaw)
    sonar_cloud[3][1] = sonarB_offset_y + sonar_maxval * math.sin(sonarB_offset_yaw)
    sonar_cloud[3][2] = sonar_height

    rospy.loginfo(" sonarB range is %s" % sonar_data.range)
    
    now = rospy.Time.now()
    
    pcloud = PointCloud2()
    try:
    	sonar_rB = sonar_data.range
    	if sonar_rB >= 0.8 or sonar_rB == 0.0:
            sonar_cloud[3][0] = sonarB_offset_x + sonar_maxval * math.cos(sonarB_offset_yaw)
            sonar_cloud[3][1] = sonarB_offset_y + sonar_maxval * math.sin(sonarB_offset_yaw)
        else:
            sonar_cloud[3][0] = sonarB_offset_x + sonar_rB * math.cos(sonarB_offset_yaw)
            sonar_cloud[3][1] = sonarB_offset_y + sonar_rB * math.sin(sonarB_offset_yaw)
            	
    except:
        sonar_data.range = 500
        return

    pcloud.header.frame_id="/base_link"
    pcloud = pc2.create_cloud_xyz32(pcloud.header, sonar_cloud)
    sonar_pub_cloud.publish(pcloud)




def sonarB_sub():
    rospy.init_node('sonarB', anonymous=True)
    #rospy.Subscriber("sonarL", Range, L_callback)
    rospy.Subscriber("sonarB", Range, B_callback)
    #rospy.Subscriber("sonarR", Range, R_callback)
    #rospy.Subscriber("sonarB", Range, B_callback)
    rospy.spin()

if __name__=='__main__':
    sonarB_sub()