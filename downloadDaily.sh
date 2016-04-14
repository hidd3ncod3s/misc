workingdir="/mnt/data/"

current_date_time="`date +%Y_%m_%d_%H_%M_%S`";

#echo $workingdir
#echo $current_date_time;

targetdir="$workingdir/Daily/$current_date_time/"

#echo $targetdir

mkdir $targetdir

wget -l1 -r -nH  --cut-dirs=2 --reject commaseparatedlist -P $targetdir "url"

rm -rf "$targetdir/unwanteddir"

