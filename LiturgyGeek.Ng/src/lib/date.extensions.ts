interface Date {
  addDays(days: number): Date;
  addMonths(months: number): Date;
  addYears(years: number): Date;
  asDateOnly(): Date;
  equals(other: Date): boolean;
}

Date.prototype.addDays = function (days: number): Date {
  var result = new Date(this);
  result.setDate(result.getDate() + days);
  return result;
}

Date.prototype.addMonths = function (months: number): Date {
  var result = new Date(this);
  result.setMonth(result.getMonth() + months);
  var date = result.getDate();
  if (date < this.getDate())
    result.setDate(0);
  return result;
}

Date.prototype.addYears = function (years: number): Date {
  var result = new Date(this);
  result.setFullYear(result.getFullYear() + years);
  var date = result.getDate();
  if (date < this.getDate())
    result.setDate(0);
  return result;
}

Date.prototype.asDateOnly = function (): Date {
  return new Date(this.getFullYear(), this.getMonth(), this.getDate());
}

Date.prototype.equals = function (other: Date): boolean {
  return this.getTime() == other.getTime();
}
