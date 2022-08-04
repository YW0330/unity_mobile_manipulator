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



def FL_callback(sonar_data):
    
    sonar_rFL = 0.0
    
    safe_range_0 = 10
    safe_range_1 = 7
    
    sonar_pub_cloud = rospy.Publisher("/sonar_cloudpoint_FL", PointCloud2, queue_size=5)
    
    sonar_height = 0
    sonar_maxval = 3.5
    
    sonarFL_offset_yaw = 0.7854 #0.7854
    sonarFL_offset_x = 0.20
    sonarFL_offset_y = 0.20

    sonar_cloud = [[100.0,0.105,0.1],[100.0,-0.105,0.1],[0.2,100.0,0.1],[0.2,-100.0,0.1]]

    sonar_cloud[0][0] = sonarFL_offset_x + sonar_maxval * math.cos(sonarFL_offset_yaw)
    sonar_cloud[0][1] = sonarFL_offset_y + sonar_maxval * math.sin(sonarFL_offset_yaw)
    sonar_cloud[0][2] = sonar_height

    rospy.loginfo(" sonarFL range is %s" % sonar_data.range)
    
    now = rospy.Time.now()
    
    pcloud = PointCloud2()
    try:
    	sonar_rFL = sonar_data.range
    	if sonar_rFL >= 1.4 or sonar_rFL == 0.0: #test 0.8
            sonar_cloud[0][0] = sonarFL_offset_x + sonar_maxval * math.cos(sonarFL_offset_yaw)
            sonar_cloud[0][1] = sonarFL_offset_y + sonar_maxval * math.sin(sonarFL_offset_yaw)
        else:
            sonar_cloud[0][0] = sonarFL_offset_x + sonar_rFL * math.cos(sonarFL_offset_yaw)
            sonar_cloud[0][1] = sonarFL_offset_y + sonar_rFL * math.sin(sonarFL_offset_yaw)
            	
    except:
        sonar_data.range = 500
        return

    pcloud.header.frame_id="/base_link"
    pcloud = pc2.create_cloud_xyz32(pcloud.header, sonar_cloud)
    sonar_pub_cloud.publish(pcloud)




def sonarFL_sub():
    rospy.init_node('sonarFL', anonymous=True)
    #rospy.Subscriber("sonarFL", Range, L_callback)
    rospy.Subscriber("sonarFL", Range, FL_callback)
    #rospy.Subscriber("sonarR", Range, R_callback)
    #rospy.Subscriber("sonarB", Range, B_callback)
    rospy.spin()

if __name__=='__main__':
    sonarFL_sub()
