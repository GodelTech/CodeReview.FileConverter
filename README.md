# Introduction 

How to build image 

docker build -t diamonddragon/file-converter -f src/ReviewItEasy.FileConverter/Dockerfile ./src

Run:

docker run -v "/d/temp:/result"   --rm diamonddragon/file-converter  run -p SonarAnalyzer.CSharp -o /result/result.yaml -j

Debug:

docker run -v "/d/temp:/result" -it --rm  --entrypoint /bin/bash  diamonddragon/file-converter 