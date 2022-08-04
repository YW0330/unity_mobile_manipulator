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



def FR_callback(sonar_data):
    
    sonar_rFR = 0.0
    
    safe_range_0 = 10
    safe_range_1 = 7
    
    sonar_pub_cloud = rospy.Publisher("/sonar_cloudpoint_FR", PointCloud2, queue_size=5)
    
    sonar_height = 0
    sonar_maxval = 3.5
    
    sonarFR_offset_yaw = -0.7854
    sonarFR_offset_x = 0.20
    sonarFR_offset_y = -0.20

    sonar_cloud = [[100.0,0.105,0.1],[100.0,-0.105,0.1],[0.2,100.0,0.1],[0.2,-100.0,0.1]]

    sonar_cloud[2][0] = sonarFR_offset_x + sonar_maxval * math.cos(sonarFR_offset_yaw)
    sonar_cloud[2][1] = sonarFR_offset_y + sonar_maxval * math.sin(sonarFR_offset_yaw)
    sonar_cloud[2][2] = sonar_height

    rospy.loginfo(" sonarFR range is %s" % sonar_data.range)
    
    now = rospy.Time.now()
    
    pcloud = PointCloud2()
    try:
    	sonar_rFR = sonar_data.range
    	if sonar_rFR >= 0.8 or sonar_rFR == 0.0:
            sonar_cloud[2][0] = sonarFR_offset_x + sonar_maxval * math.cos(sonarFR_offset_yaw)
            sonar_cloud[2][1] = sonarFR_offset_y + sonar_maxval * math.sin(sonarFR_offset_yaw)
        else:
            sonar_cloud[2][0] = sonarFR_offset_x + sonar_rFR * math.cos(sonarFR_offset_yaw)
            sonar_cloud[2][1] = sonarFR_offset_y + sonar_rFR * math.sin(sonarFR_offset_yaw)
            	
    except:
        sonar_data.range = 500
        return

    pcloud.header.frame_id="/base_link"
    pcloud = pc2.create_cloud_xyz32(pcloud.header, sonar_cloud)
    sonar_pub_cloud.publish(pcloud)




def sonarFR_sub():
    rospy.init_node('sonarFR', anonymous=True)
    #rospy.Subscriber("sonarL", Range, L_callback)
    rospy.Subscriber("sonarFR", Range, FR_callback)
    #rospy.Subscriber("sonarFR", Range, R_callback)
    #rospy.Subscriber("sonarB", Range, B_callback)
    rospy.spin()

if __name__=='__main__':
    sonarFR_sub()
