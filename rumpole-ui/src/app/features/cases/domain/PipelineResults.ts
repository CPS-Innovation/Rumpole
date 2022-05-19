import { PdfDocument } from "./PdfDocument";

export type PipelineResults = {
  transationId: string;
  documents: PdfDocument[];
};
