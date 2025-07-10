# FROM harbor.wistron.com/base_image/ng-cli-karma:15.2.7 AS build
FROM harbor.wistron.com/base_image/ng-cli-karma:16.1.0 AS build
# FROM harbor-k8s.wzs.wistron.com.cn/base_image/ng-cli-karma AS build
WORKDIR /web/ui
COPY ./web/ui .

RUN if [ -f package-lock.json ]; then rm -r package-lock.json; fi && npm cache clean -f


# 添加重试机制
RUN npm i --legacy-peer-deps || npm i --legacy-peer-deps || npm i --legacy-peer-deps

RUN ng config -g cli.warnings.versionMismatch false
RUN ng build --configuration production

FROM harbor.wistron.com/base_image/nginx-unprivileged:1.23.2 AS base

LABEL maintainer="ML3320@wistron.com"
ENV TZ=Asia/Shanghai
ENV LANG=en_US.UTF-8
COPY --from=build /web/ui/dist/angular /usr/share/nginx/html/
EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
