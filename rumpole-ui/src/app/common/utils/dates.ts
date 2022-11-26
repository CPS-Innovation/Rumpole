import { format, parseISO } from "date-fns";

export const CommonDateTimeFormats = {
  ShortDate: "dd/MM/yyyy",
  ShortDateTextMonth: "dd MMM yyyy",
  ShortDateFullTextMonth: "dd LLLL yyyy",
};

export const formatDate = (isoDateString: string, dateTimeFormat: string) =>
  isoDateString && format(parseISO(isoDateString), dateTimeFormat);
