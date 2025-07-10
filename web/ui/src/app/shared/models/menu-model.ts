export interface MenuModel {
  title: string;
  icon?: string;
  open?: boolean;
  selected?: boolean;
  url?: string;
  paddingLeft: number;
  children?: MenuModel[];
}
