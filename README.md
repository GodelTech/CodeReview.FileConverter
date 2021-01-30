# Introduction 

How to build image 

docker build -t godeltech/codereview.file-converter:0.0.1 -f src/CodeReview.FileConverter/Dockerfile ./src
docker image tag godeltech/codereview.file-converter:0.0.1 godeltech/codereview.file-converter:latest
docker push godeltech/codereview.file-converter:latest
docker push godeltech/codereview.file-converter:0.0.1

Run:

docker run -v "/d/temp:/result"   --rm godeltech/codereview.file-converter  run -p SonarAnalyzer.CSharp -o /result/result.yaml -j

Debug:

docker run -v "/d/temp:/result" -it --rm  --entrypoint /bin/bash  godeltech/codereview.file-converter 