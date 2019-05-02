import subprocess
import os
import socket
import csv
import time,sched
import RPi.GPIO as GPIO
import glob
import logging
import requests
import json
from errno import ENETUNREACH
headers={'Content-type': 'application/json'}
s = sched.scheduler(time.time, time.sleep)
from threading import Timer
from suds.client import Client
URL_get_data = "http://192.168.43.196:5010/get_data" # change the ip to current ip of flask along with its port number.
URL_update_mois_data="http://192.168.43.196:5010/update_mois_data" # change the ip to current ip of flask along with its port number
URL_delete_ip="http://192.168.43.196:5010/delete_ip" # change the ip to current ip of flask along with the port number.
#client = Client(url)
TRIG=23
ECHO=24
mois = 0
RELAY_1 = 25
RELAY_2=16
GPIO.setwarnings(False)
GPIO.setmode(GPIO.BCM)
GPIO.setup(RELAY_1,GPIO.OUT)
GPIO.setup(RELAY_2,GPIO.OUT)
GPIO.setup(TRIG,GPIO.OUT)
GPIO.setup(ECHO,GPIO.IN)
moisture_threshold_low=400
moisture_threshold_high=80
water_tank_height=25
water_tank_status = ""
distance = 0
raspberry_id=1

def read_distance():
    try:
        #water_tank_height=8
        water_level=0
        print "Water level sensor testing"
        GPIO.output(TRIG, False)
        print "Waiting For Sensor To Settle"
        time.sleep(2)
        GPIO.output(TRIG, True)

        time.sleep(0.00001)

        GPIO.output(TRIG, False)
        while GPIO.input(ECHO)==0:
                pulse_start = time.time()
        while GPIO.input(ECHO)==1:
                pulse_end = time.time()
        pulse_duration = pulse_end - pulse_start
        distance = pulse_duration * 17150
        distance = round(distance, 2)
        print distance
        final_distance = distance
        print int(100-((100*distance)/water_tank_height))
        return final_distance
    except:
        return 0 
        
    
##def run_status():
##    try:
##        data="rashmipawar921@gmail.com"
##        json_data = json.dumps(data)
##        r = requests.get(url = URL_delete_ip, json = json_data, headers = headers)
##        print "running alive_status_nmcu.py"
##        execfile("/home/pi/Desktop/alive_status_nmcu.py")
##    except:
##        pass
    
def run_sensor():
    
    temp=0
    raspberry_id="1"
    water_tank_level=30
    water_pump_status="off"
    send_noti="1"
    water_level=20
    ip_list=[]
    f=open("/home/pi/Desktop/AICSMM/nmcu_ip.csv","r")
    file_contents=csv.reader(f)
    ip_list=list(file_contents)
    #print type(ip_list)

    moisture_readings={}

    for i in range(len(ip_list)):

            #ip=i.split(",")
            #print ip_list[i]
        try:
            deadline = time.time() + 5.0
            UDP_IP=ip_list[i][1]
            #print ip
            UDP_PORT=1885
            MESSAGE="aiscmm_smart_irrigation_169.254.152.165_sensor_data"
            
            print "UDP target ip:", UDP_IP
            print "udp target port:", UDP_PORT
            print "msg:",MESSAGE

            sock=socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
            try:
                sock.sendto(MESSAGE,(UDP_IP, UDP_PORT))
                print "Packet send"
            except IOError as e:
                if e.errno==ENETUNREACH:
                    pass
            sock.settimeout(deadline-time.time())
            chunks=[]
            bytes_recd=0
            chunks=sock.recv(4096)
            print chunks
            #print type(chunks)
            chunks_list=[]
            chunks_list=chunks.split(",")
            #print chunks_list
            temp=chunks_list[0]
            mois = chunks_list[2]
            #print type(mois)
            #print mois
            moisture_readings[i]=float(mois)
            raspberry_id="1"
            water_pump_status="off"
            send_noti="1"
            #flag for moisture
            flag = 2 #water motor off
        except socket.timeout:
            print "time out"
            moisture_readings[i]=2000
            continue

    
    #print(moisture_readings)
    no_of_pipes=2
    pipe_list1=[]
    pipe_list2=[]
    for i in moisture_readings:
        pipe_no=(i+1)%no_of_pipes;
        if pipe_no==1:
            pipe_list1.append(moisture_readings[i])
        else:
            pipe_list2.append(moisture_readings[i])
        
    mois_avg1=0;
    mois_avg2=0;
    faulty_count1=0
    faulty_count2=0
    mois_avg_list=[]
    for i in range(len(pipe_list1)):
        if pipe_list1[i]==2000:
            faulty_count1=faulty_count1+1
            continue
        else:
            mois_avg1=mois_avg1+pipe_list1[i]
        mois_avg1=mois_avg1/(len(pipe_list1)-faulty_count1)
    mois_avg_list.append(mois_avg1)
    
    for i in range(len(pipe_list2)):
        if pipe_list2[i]==2000:
            faulty_count2=faulty_count2+1
            continue
        else:
            mois_avg2=mois_avg2+pipe_list2[i]
        mois_avg2=mois_avg2/(len(pipe_list2)-faulty_count2)
    mois_avg_list.append(mois_avg2)
    
    print mois_avg_list
    
    water_tank_status = "off"
    #RELAY_1=25
    #RELAY_2=16
    #print RELAY
    #print read_distance()
    try:
    
        for i in range(len(mois_avg_list)):
            try:        
                if(mois_avg_list[i]>moisture_threshold_low):
                    try:
			distance = read_distance()
                    	water_tank_level=distance
			print(distance)
		    except:
			print("cannot find distance...................")
			distance = 10000
                    print(distance, water_tank_height)
                    if(distance<water_tank_height):
                        print("inside the pump")
                        if i==0:
			    try:
                            	GPIO.output(RELAY_1,True)
                    
                            	water_tank_status = "motor on"
                            	print water_tank_status
                            	#result=client.service.update_farm_status(temp,mois,raspberry_id,water_tank_level,water_tank_status,send_noti)
                            	try:
					data={"raspberry_id":raspberry_id,"pump_id":i+1,"mois":mois_avg_list[i]}
                            		print data
                            
                                        json_data = json.dumps(data)
                                        r = requests.get(url = URL_update_mois_data, json = json_data, headers = headers)
                            		
					print "data is updated to db"
                            		"""t = time.time() + 5
                            		while(time.time() != t):
                                		continue"""

                            except:
					pass
                            	time.sleep(10)
                            	GPIO.output(RELAY_1, False)
                            	water_tank_status = "off"
                            	print water_tank_status
				
			    except:
                        	pass
                        else:
			    try:
                            	GPIO.output(RELAY_2,True)
                        
                            	water_tank_status = "motor on"
                            	print water_tank_status
                            	#result=client.service.update_farm_status(temp,mois,raspberry_id,water_tank_level,water_tank_status,send_noti)
                            	try:
					data={"raspberry_id":raspberry_id,"pump_id":i+1,"mois":mois_avg_list[i]}
                            
                            		json_data = json.dumps(data)
                            		r = requests.get(url = URL_update_mois_data, json = json_data, headers = headers)
                            		print "data is updated to db"
                            		"""t = time.time() + 5
                            		while(time.time() != t):
                                		continue"""
                            	except:
					pass

				time.sleep(10)
                            	GPIO.output(RELAY_2, False)
                            	water_tank_status = "off"
                            	print water_tank_status
				
                            except:
				pass
                    else:
                        #water_tank_status = "off"
                        print ("water_tank_status pehla off")
			
                        
                else:
                    #water_tank_status = "off"
                    print ("water_tank_status dusra off")
		    
            except:
		print("pump ke bahar")
                pass
        #result=client.service.update_farm_status(temp,mois,raspberry_id,water_tank_level,water_tank_status,send_noti)
            
	    print("trying for flask...........")
	    try:
	    	data={"temp":temp,"mois":mois_avg_list[i],"raspberry_id":raspberry_id,"water_tank_level":water_tank_level,"water_tank_status":water_tank_status,"send_noti":send_noti}
                        
            	json_data = json.dumps(data)
            	r = requests.get(url = URL_get_data, json = json_data, headers = headers)
            	print "data is updated to db"
	    except:
		print("Flask ke bahar")
		pass
    except:
        #continue 
	pass
    print("done run sensor...............................")
while(1):
    print("new iloop")
#os.system("ping_test.py")
    #Timer(110, run_status, ()).start()
    #time.sleep(1)
    #Timer(30, run_sensor, ()).start()
    s.enter(30, 1, run_sensor, ())
    print("done runsensor")
   # s.enter(2, 1, run_status, ())
    #print("done runstatus")
    s.run()
    #time.sleep(120)


