import { MatomoTracker } from "ngx-matomo";


export function MatomoSetUserid(customTitle?: string) {
  const matomoTracker: MatomoTracker = new MatomoTracker();
  return (
    target: any,
    propertyKey: string,
    propertyDescriptor: PropertyDescriptor
  ) => {
    customTitle = customTitle || `${target.constructor.name}#${propertyKey}()`;

    const func = propertyDescriptor.value;
    propertyDescriptor.value = function (...args: any[]) {
      matomoTracker.setUserId(customTitle);
      func.apply(this, args);
    };
  };
}