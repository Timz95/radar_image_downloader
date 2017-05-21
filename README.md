# radar_image_downloader
Simple commandline program that downloads radar images from the Buienradar API.


# Usage

Buienradar.exe -s [dd-mm-yyyy] -e [dd-mm-yyyy] -o [path] -i [interval]

Required parameters:

  -s : Specifies the date from which the program has to start downloading images from.
  -e : Specified the enddate. The program will stop downloading images if it reaches this date.
  -o : Output folder of the images. If the folder does not exist, it will be created.

Optional parameters:

  -i : Specifies the interval between the radar images in minutes. The interval must be divisible by 5.
