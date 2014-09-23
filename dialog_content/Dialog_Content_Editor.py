print "Importing Python dependencies and setting local path..."

import os
import sys
import subprocess

if getattr(sys, 'frozen', False):
    BASE_DIR = sys._MEIPASS
else:
    BASE_DIR = os.path.dirname(__file__)
print BASE_DIR

os.chdir(BASE_DIR)

sys.path.append(BASE_DIR)
sys.path.append(BASE_DIR + "/DLLs")
sys.path.append(BASE_DIR + "/Lib")
sys.path.append(BASE_DIR + "/Lib/plat-win")
sys.path.append(BASE_DIR + "/Lib/lib-tk")
sys.path.append(BASE_DIR + "/Scripts")
sys.path.append(BASE_DIR + "/Lib/site-packages")

import threading
import thread

import decimal
import contextlib
import UserList
import UserString
import commands
import robotparser
import Cookie
import json
import hmac
import cgi
import htmlentitydefs
import HTMLParser
from logging import handlers
import ConfigParser
import Queue
import xml.dom.minidom
import wsgiref
from wsgiref import simple_server
import sqlite3


def launch_viewer():
    subprocess.call(os.path.join(BASE_DIR, 'Chromium', 'Dialog Content Editor Viewer.exe'))
    print "Closing application!"
    thread.interrupt_main()

print "Preparing Django environment..."

os.environ.setdefault("DJANGO_SETTINGS_MODULE", "dialog_content.settings")

exec("from django.conf import settings")
exec("from django.core import management")

browser = threading.Timer(3, launch_viewer)
browser.start()

management.call_command('syncdb', interactive=False)
management.call_command('runproductionserver', port=8000, serve_static='collect')