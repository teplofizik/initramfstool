# initramfs editing
Tool for fast modifying initramfs images (fix web, configs etc) from Windows without using Linux build environment.
Only ascii-cpio (070701) archives with gzip compression.

## List files
```
InitramfsTool.exe ramfs=./example/initramfs.bin.SD
```
will produce such output:
```
sbin/pidof: rwxrwxrwx m:a1ff in:33845765 links:1 maj:252 min:18 rmaj:0 rmin:0
sbin/route: rwxrwxrwx m:a1ff in:33845769 links:1 maj:252 min:18 rmaj:0 rmin:0
sbin/rmmod: rwxrwxrwx m:a1ff in:33845768 links:1 maj:252 min:18 rmaj:0 rmin:0
sbin/syslogd: rwxrwxrwx m:a1ff in:33845855 links:1 maj:252 min:18 rmaj:0 rmin:0
sbin/modprobe: rwxrwxrwx m:a1ff in:33845764 links:1 maj:252 min:18 rmaj:0 rmin:0
sbin/udhcpc: rwxrwxrwx m:a1ff in:33845856 links:1 maj:252 min:18 rmaj:0 rmin:0
tmp: rwxr-xr-x m:41ed in:33845654 links:2 maj:252 min:18 rmaj:0 rmin:0
mnt: rwxr-xr-x m:41ed in:33845647 links:2 maj:252 min:18 rmaj:0 rmin:0
```
## Extract files
```
InitramfsTool.exe ramfs=./example/initramfs.bin.SD extract=./extracted/
```
All directories and regular files will be extracted (for content editing).

## Modify content
```
InitramfsTool.exe ramfs=./example/initramfs.bin.SD root=./root/ commands=./commands.txt out=./initramfs.bin.SD
```
Arguments: 
| Argument | Description | Example value |
| ------------- | ------------- |------------- |
| ramfs  | path to initramfs file  |./example/initramfs.bin.SD |
| root  | path to directory with added and modified files [not required] | ./root/ |
| commands | path to file with commands (change mode, remove file) [not required] | ./commands.txt |
| out | path to processed initramfs with modified content | ./initramfs.bin.SD |

Commands file example:
```
rm www/pages/kernelLog.html
chmod www/pages/test.txt rwxrwx---
```
Available commands:
| Command | Args | Descripton |
| ------------- | ------------- |------------- |
| rm | [path] | Remove file with specified path |
| chmod | [path] [mode] | Change mode of file ('rwxrwxrwx' format: user,group,other) |
| chown | [path] [uid] | Change user id of file |
| group | [path] [gid] | Change group id of file |
| chown | [path] [uid]:[gid] | Change user id and group id of file |
| dir | [path] [mode] [uid] [gid] | Add directory with provided uid/gid/mode |
| file | [path] [local] [mode] [uid] [gid] | Add file with content from exists file with provided uid/gid/mode |
| file | [path] [local] | Update file with content from exists file |
| slink | [path] [to] [mode] [uid] [gid] | Add symbolic link to provided file or dir with provided uid/gid/mode |
| slink | [path] [to] | Update symbolic link to provided file or dir |
| include | [path] | Load another commands file from relative from that or absolute path |
| echo | [message] | Print message to console |

[deprecated]
Content of image files will be replaced by content on file from root directory. Files that not exists in image, but exists in root directory will be added to image.

Root directory content example:
```
etc\init.d\pgnand.sh
www\pages\index.html
www\pages\test.txt
www\pages\test\test2.txt
```

Pipeline: load image => unpack cpio => process cpio data => modify content => add files => process commands (remove, chmod) => build cpio data => pack cpio => generate modified initramfs

# CPIO editing
## List files
```
InitramfsTool.exe cpio=./example/Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio
```
will produce such output:
```
usr/bin/nohup: m:a1ff in:398764 links:1 maj:8 min:1 rmaj:0 rmin:0
usr/bin/dbclient: m:a1ff in:396446 links:1 maj:8 min:1 rmaj:0 rmin:0
usr/bin/tr: m:a1ff in:401452 links:1 maj:8 min:1 rmaj:0 rmin:0
usr/bin/tail: m:a1ff in:401373 links:1 maj:8 min:1 rmaj:0 rmin:0
usr/bin/cmp: m:a1ff in:398589 links:1 maj:8 min:1 rmaj:0 rmin:0
usr/bin/deallocvt: m:a1ff in:398614 links:1 maj:8 min:1 rmaj:0 rmin:0
usr/bin/microcom: m:a1ff in:398716 links:1 maj:8 min:1 rmaj:0 rmin:0
```

## Extract files
```
InitramfsTool.exe cpio=./example/Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio extract=./extracted/
```
All directories and regular files will be extracted (for content editing).

## Modify content
```
InitramfsTool.exe cpio=./example/Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio root=./root/ commands=./commands.txt out=./Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio
```
Arguments: 
| Argument | Description | Example value |
| ------------- | ------------- |------------- |
| cpio  | path to cpio file  |./example/Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio |
| root  | path to directory with added and modified files [not required] | ./root/ |
| commands | path to file with commands (change mode, remove file) [not required] | ./commands.txt |
| out | path to processed cpio with modified content | ./Angstrom-antminer_m-eglibc-ipk-v2013.06-beaglebone.rootfs.cpio |

All logic same with initramfs part.