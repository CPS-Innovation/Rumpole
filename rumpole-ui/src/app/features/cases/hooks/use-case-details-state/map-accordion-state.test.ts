import { mapAccordionState } from "./map-accordion-state";
import { CaseDocument } from "../../domain/CaseDocument";
import { ApiResult } from "../../../../common/types/ApiResult";
import { MappedCaseDocument } from "../../domain/MappedCaseDocument";

jest.mock("./document-category-definitions", () => ({
  categoryNamesInPresentationOrder: ["category-a", "category-b"],
}));

describe("mapAccordionState", () => {
  it("can return a loading status when the api result is  'loading'", () => {
    const apiResult: ApiResult<CaseDocument[]> = {
      status: "loading",
    };

    const result = mapAccordionState(apiResult);

    expect(result).toEqual({ status: "loading" });
  });

  it("can return a loading status when the api is result is 'failed'", () => {
    const apiResult: ApiResult<CaseDocument[]> = {
      status: "failed",
      error: null,
      httpStatusCode: undefined,
    };

    const result = mapAccordionState(apiResult);

    expect(result).toEqual({ status: "loading" });
  });

  it("can map from an api result to accordion input shape", () => {
    const apiResult: ApiResult<MappedCaseDocument[]> = {
      status: "succeeded",
      data: [
        {
          documentId: "1",
          tabSafeId: "d0",
          category: "category-a",
          fileName: "foo",
          cmsDocType: {
            code: "MG11",
            name: "MG11 File",
          },
          createdDate: "2020-01-01",
        },
        {
          documentId: "2",
          tabSafeId: "d1",
          category: "category-b",
          fileName: "bar",
          cmsDocType: {
            code: "MG12",
            name: "MG12 File",
          },
          createdDate: "2020-01-02",
        },
      ],
    };

    const result = mapAccordionState(apiResult);

    expect(result).toEqual({
      status: "succeeded",
      data: [
        {
          sectionId: "category-a",
          sectionLabel: "category-a",
          docs: [
            {
              documentId: "1",
              tabSafeId: "d0",
              category: "category-a",
              fileName: "foo",
              cmsDocType: {
                code: "MG11",
                name: "MG11 File",
              },
              createdDate: "2020-01-01",
            },
          ],
        },
        {
          sectionId: "category-b",
          sectionLabel: "category-b",
          docs: [
            {
              documentId: "2",
              tabSafeId: "d1",
              category: "category-b",
              fileName: "bar",
              cmsDocType: {
                code: "MG12",
                name: "MG12 File",
              },
              createdDate: "2020-01-02",
            },
          ],
        },
      ],
    } as ReturnType<typeof mapAccordionState>);
  });
});