# ffmedia
The next iteration of the ffmediaelement project.


## Coding Rules and Conventions

### The ```FFmpeg``` Namespace

The classes in this namespace serve a thin wrappers for native data structures. As such, here
are the coding conventions for this namespace and its usage.

 - Tracked references are those that are allocated and freed by the class itself.
 - Native references are those that are not allocated by user code but can be wrapped.
 - ```long``` timestamps must be made nullable and be null when their value is ```ffmpeg.AV_NOPTS_VALUE```
 - ```AVRational``` must be made nullable and be null when their value is 0/1
 - Do not write property or method wrappers using null pointer or zero address checks. The thin wrappers should
   minimize performance penalties at all costs. It is the userr's responsibility to check for ```IsDisposed```
   and/or ```IsEmpty``` properties of tracked and native references respectively.
 - For method wrappers, add a ```<remarks />``` section and reference the native method call(s) used.
