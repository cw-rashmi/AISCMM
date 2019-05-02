import os
import socket
import csv
import time,sched
import glob
import logging
import json
import requests
from threading import Timer
from suds.client import Client

#creating the instance of schedular
s = sched.scheduler(time.time, time.sleep)

headers={'Content-type': 'application/json'}
#URL of flask server to update the nodemcu status
URL = "http://192.168.43.196:5010/insert_ip"

faulty_nmcu={}

def run_sensor_status():   
    ip_list=[]
    UDP_PORT=1885
    MESSAGE="aiscmm_smart_irrigation_169.254.152.165_sensor_status"
    #reading the nodemcu ip address and id from csv file
    f=open("/home/pi/Desktop/AICSMM/nmcu_ip.csv","r")
    file_contents=csv.reader(f)
    ip_list=list(file_contents)
    
    for i in range(len(ip_list)):
        chunks=[]
        chunks_list=[]
        bytes_recd=0
        #setting deadline for socket of 5 seconds
        deadline = time.time() + 5.0
        UDP_IP=ip_list[i][1]
        
        print "UDP target ip:", UDP_IP
        print "udp target port:", UDP_PORT
        print "msg:",MESSAGE
        try:           
            #creating socket for each nodemcu to request status
            sock=socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
            sock.sendto(MESSAGE,(UDP_IP, UDP_PORT))
            print "Packet send"
            #socket will exist for only 5 seconds
            sock.settimeout(deadline-time.time())
            chunks=sock.recv(4096)
            chunks_list=chunks.split(",")
            st=chunks_list[0]
            print st
            
            #if the nodemcu is responding other than alive
            if st!="alive":
                if ip_list[i][1] in faulty_nmcu.keys():
                        faulty_nmcu[ip_list[i][1]]=faulty_nmcu[ip_list[i][1]]+1
                    
                else:
                        faulty_nmcu[ip_list[i][1]]=1
            elif st=="alive":
                if ip_list[i][1] in faulty_nmcu.keys():
                    faulty_nmcu.pop(ip_list[i][1])
                    print(faulty_nmcu)
            
        except socket.timeout:
            print "time out"
            if ip_list[i][1] in faulty_nmcu.keys():
                    faulty_nmcu[ip_list[i][1]]=faulty_nmcu[ip_list[i][1]]+1
                    
            else:
                    faulty_nmcu[ip_list[i][1]]=1
            continue
        
    print faulty_nmcu
    
    for key,value in faulty_nmcu.items():
        #if any nodemcu is detected faulty more than 3 times then it is declared as dead and results are updated to server
        if value>3:
            id=[x[0] for x in ip_list if x[1]==key]
            data={"nodemcu_id":id[0],"raspberry_id":1,"nodemcu_ip":key,"raspberry_ip":"169.254.152.165","email":"rashmipawar921@gmail.com"}
            print(data)       
            json_data = json.dumps(data)
            try:
                    r = requests.get(url = URL, json = json_data, headers = headers, timeout=5)
            except requests.exceptions.RequestException as e:
                pass
    print "data is updated to db"
    
#infinite loop that will schedule the execution after each 5 seconds
while(1):
    print("starting run_sensor_status.....................")    
    s.enter(5, 1, run_sensor_status, ())
    print("done status....................")
    s.run()
    

