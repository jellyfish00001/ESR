export const environment = {
  SERVER_URL: `./`,
  production: false,
  VERSION: require('../../package.json').version,
  supportedLocale:['en','zh-TW','zh_CN'],
  clientId: "7e520eb7-5705-403b-a7f3-3a9542f5bae9",// 替换为你的 AAD 应用程序的客户端 ID
  authority: "https://login.microsoftonline.com/de0795e0-d7c0-4eeb-b9bb-bc94d8980d3b",// 替换为你的租户 ID
};


