import { AsyncResult } from "../../../../common/types/AsyncResult";
import { CaseDocument } from "../../domain/CaseDocument";
import { MappedCaseDocument } from "../../domain/MappedCaseDocument";
import { mapDocumentsState } from "./map-documents-state";

jest.mock("./document-category-definitions", () => ({
  getCategory: (item: CaseDocument) => "category" + item.documentId,
}));

jest.mock("../../logic/get-file-name-without-extension", () => ({
  getFileNameWithoutExtension: (filename: string) => filename + "!",
}));

describe("mapDocumentsState", () => {
  it("can leave the input alone if status is loading", () => {
    const input = { status: "loading" } as AsyncResult<CaseDocument[]>;

    const result = mapDocumentsState(input);

    expect(result).toBe(input);
  });

  it("can map CaseDocuments to MappedCaseDocuments", () => {
    const doc1 = { documentId: "0", fileName: "foo" } as CaseDocument;
    const doc2 = { documentId: "1", fileName: "bar" } as CaseDocument;

    const input = {
      status: "succeeded",
      data: [doc1, doc2],
    } as AsyncResult<CaseDocument[]>;

    const expectedResult = {
      status: "succeeded",
      data: [
        {
          ...doc1,
          tabSafeId: "d0",
          category: "category0",
          fileName: "foo",
          presentationFileName: "foo!",
        },
        {
          ...doc2,
          tabSafeId: "d1",
          category: "category1",
          fileName: "bar",
          presentationFileName: "bar!",
        },
      ] as MappedCaseDocument[],
    };

    const result = mapDocumentsState(input);

    expect(result).toEqual(expectedResult);
  });
});
