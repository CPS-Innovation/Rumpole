export class ApiError extends Error {
  public readonly name: string = "API_ERROR";
  public readonly code: number;

  constructor(message: string, { status, statusText }: Response) {
    super(
      `An error ocurred contacting the server: ${message}; status - ${statusText} (${status})`
    );
    this.code = status;
  }
}
