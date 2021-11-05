import { format, parseISO } from "date-fns";

export const commonDateTimeFormats = {
  shortDate: "dd/MM/yyyy",
};

export const formatISODate = (isoDateString: string, dateTimeFormat: string) =>
  format(parseISO(isoDateString), dateTimeFormat);
