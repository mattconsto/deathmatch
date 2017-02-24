#!/bin/bash

for file in *.wav; do
  name=${file%%.wav}
  echo $name
  lame -V0 -h -b 256 $name.wav $name.mp3
done
