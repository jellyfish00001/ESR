import { Injectable } from "@angular/core";
import { DES, mode, pad, enc } from 'crypto-js';


@Injectable()
export class CryptoService {
  private keyHex: string;//密钥
  constructor() {
    this.keyHex = enc.Utf8.parse('ERSand2022');
  }

  /**
   * DES加密
   * @param {string} data 待加密字符串
   * @description 用于对字符串加密
   * @return {String} 加密后的字符串
   */
  encrypt(data: string): string {
    let encrypted = DES.encrypt(data, this.keyHex, {
      mode: mode.ECB,
      padding: pad.Pkcs7
    })
    return encrypted.toString();
  }

  /**
   * DES解密
   * @param {String} data 待解密字符串
   * @description 用于对加密串的解密
   * @return {String} 解密后的字符串
   */
  decrypt(data: string): string {
    let decrypted = DES.decrypt(data, this.keyHex, {
      mode: mode.ECB,
      padding: pad.Pkcs7
    })
    return enc.Utf8.stringify(decrypted);
  }
}