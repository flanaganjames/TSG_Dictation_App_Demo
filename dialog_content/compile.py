import sys
import os


def query_yes_no(question, default='yes'):
    valid = {'yes': True, 'y': True, "no": False, 'n': False}

    if default is None:
        prompt = " [y/n] "
    elif default in ['yes', 'y']:
        prompt = " [Y/n] "
    elif default in ['no', 'n']:
        prompt = " [y/N] "
    else:
        raise ValueError("Invalid default answer!")

    while True:
        sys.stdout.write('\n' + question + prompt)

        choice = raw_input().lower()

        if default is not None and choice == '':
            return valid[default]
        elif choice in valid:
            return valid[choice]
        else:
            sys.stdout.write("Please respond with 'yes' or 'no'.\n")


print "You should only run this compile script if you are in a CLEAN VirtualEnv!\n" \
      "The VirtualEnv will be deployed with the project, so make sure it ONLY has the REQUIRED dependencies installed!"
if not query_yes_no("Are you in a CLEAN Python Environment?", default='no'):
    sys.exit()

##########
# Cleanup root folder
# This is done before all other imports since we will need to clean out some pyd files that get loaded by python
##########
print "Cleaning base directory..."

BASE_DIR = os.path.dirname(os.path.realpath(__file__))
for file in os.listdir(BASE_DIR):
    if file.endswith(".pyd"):
        os.remove(file)

from distutils.dir_util import copy_tree, remove_tree

folders_to_delete = ['build', 'dist', 'Lib', 'Scripts']
for folder in folders_to_delete:
    try:
        remove_tree(os.path.join(BASE_DIR, folder))
    except:
        pass

# Remaining imports
import errno
import subprocess
from subprocess import CalledProcessError
import shutil

import zipfile

##########
# Gather Path info and setup environment
##########
print "Noting directory structure."

print "Project:", BASE_DIR
os.chdir(BASE_DIR)

PYTHON_INSTALL = os.path.dirname(sys.executable)
if 'scripts' in PYTHON_INSTALL.lower():
    # We are in a virtualenv and need to go up one more directory
    PYTHON_INSTALL = os.path.dirname(PYTHON_INSTALL)
print "Python:", PYTHON_INSTALL

os.environ.setdefault("DJANGO_SETTINGS_MODULE", "dialog_content.settings")
from django.core import management

##########
# Copy all project dependencies into the root for deployment
# This is things like matplotlib, django, pandas...
# Currently, we are expecting that the executing environment is a clean environment only used for SpreadModel
# If you run this in your normal python install (not dedicated VirtualEnv), then excess dependencies will be copied
##########
print "Copying dependencies..."

try:
    copy_tree(os.path.join(PYTHON_INSTALL, 'Lib'), os.path.join(BASE_DIR, 'Lib'))
    copy_tree(os.path.join(PYTHON_INSTALL, 'Scripts'), os.path.join(BASE_DIR, 'Scripts'))
    pass
except OSError as e:
    if e.errno == errno.ENOTDIR:
        raise OSError("Unknown site_packages and/or Python Lib configuration. Found file, expected folder!")
    else:
        raise

##########
# Compile the Python Entry Point and Python Executable
# Then zip everything up into an archive for sending away
##########
print "Compiling Dialog_Content_Editor.py and Python into executable..."

PYINSTALLER = os.path.join(PYTHON_INSTALL, 'Scripts', 'pyinstaller.exe')
subprocess.call(PYINSTALLER + ' --noconfirm Dialog_Content_Editor.spec', shell=True)

##########
# Prepare project root
# Collect static
# Zip up all the files
##########
print "Preparing project root for deployment"

management.call_command('collectstatic', interactive=False, clear=True)
print "\n"
copy_tree(os.path.join(BASE_DIR, 'dist', 'Dialog_Content_Editor'), BASE_DIR)

print "Zipping output into deployable..."
deployable_path = os.path.join(BASE_DIR, 'TSG_Dialog_Content_Editor.zip')
if os.path.exists(deployable_path):
    os.remove(deployable_path)

deployable_path_len = len(os.path.dirname(deployable_path).rstrip(os.sep)) + 1

folders_to_ignore = ['dist', 'build', '.idea', 'cef', ]
folders_to_ignore = [os.path.join(BASE_DIR, x) for x in folders_to_ignore]

files_to_ignore = ['TSG_Dialog_Content_Editor.zip', ]

with zipfile.ZipFile(deployable_path, mode='w', compression=zipfile.ZIP_DEFLATED) as deployable:
    for root, dirs, files in os.walk(BASE_DIR):
        if root in folders_to_ignore:
            continue
        else:
            for filename in files:
                if filename in files_to_ignore:
                    continue
                else:
                    path = os.path.join(root, filename)
                    entry = path[deployable_path_len:]
                    deployable.write(os.path.join('TSG_Dialog_Content_Editor', path), entry)

print "Done."