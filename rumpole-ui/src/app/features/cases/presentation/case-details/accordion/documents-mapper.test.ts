import { documentsMapper } from "./documents-mapper";
import { UseApiResult } from "../../../../../common/hooks/useApi";
import { CaseDocument } from "../../../domain/CaseDocument";
import { Section } from "./types";

describe("accordion documentsmapper", () => {
  it("can return false when the api is result is 'loading'", () => {
    const apiResult: UseApiResult<CaseDocument[]> = {
      status: "loading",
    };

    const result = documentsMapper(apiResult);

    expect(result).toBe(false);
  });

  it("can return false when the api is result is 'failed'", () => {
    const apiResult: UseApiResult<CaseDocument[]> = {
      status: "failed",
      error: null,
      httpStatusCode: undefined,
    };

    const result = documentsMapper(apiResult);

    expect(result).toBe(false);
  });

  it("can map from an api result to accordion input shape", () => {
    const apiResult: UseApiResult<CaseDocument[]> = {
      status: "succeeded",
      data: [
        {
          documentId: "1",
          fileName: "foo",
          cmsDocType: {
            code: "MG11",
            name: "MG11 File",
          },
          createdDate: "2020-01-01",
        },
        {
          documentId: "2",
          fileName: "bar",
          cmsDocType: {
            code: "MG12",
            name: "MG12 File",
          },
          createdDate: "2020-01-02",
        },
      ],
    };

    const result = documentsMapper(apiResult);

    expect(result).toEqual([
      { sectionId: "Reviews", sectionLabel: "Reviews", docs: [] },
      { sectionId: "Case overview", sectionLabel: "Case overview", docs: [] },
      { sectionId: "Statements", sectionLabel: "Statements", docs: [] },
      { sectionId: "Exhibits", sectionLabel: "Exhibits", docs: [] },
      { sectionId: "Forensics", sectionLabel: "Forensics", docs: [] },
      {
        sectionId: "Unused materials",
        sectionLabel: "Unused materials",
        docs: [],
      },
      { sectionId: "Defendant", sectionLabel: "Defendant", docs: [] },
      {
        sectionId: "Court preparation",
        sectionLabel: "Court preparation",
        docs: [],
      },
      { sectionId: "Communications", sectionLabel: "Communications", docs: [] },
      // have unknown last so it can scoop up any unmatched documents
      {
        sectionId: "Uncategorised",
        sectionLabel: "Uncategorised",
        docs: [
          {
            documentId: "1",
            fileName: "foo",
            cmsDocType: {
              code: "MG11",
              name: "MG11 File",
            },
            createdDate: "2020-01-01",
          },
          {
            documentId: "2",
            fileName: "bar",
            cmsDocType: {
              code: "MG12",
              name: "MG12 File",
            },
            createdDate: "2020-01-02",
          },
        ],
      },
    ] as Section[]);
  });

  it("can hide the uncategorised section if there are no uncategorised documents", () => {
    const apiResult: UseApiResult<CaseDocument[]> = {
      status: "succeeded",
      data: [],
    };

    const result = documentsMapper(apiResult);

    expect(result).toEqual([
      { sectionId: "Reviews", sectionLabel: "Reviews", docs: [] },
      { sectionId: "Case overview", sectionLabel: "Case overview", docs: [] },
      { sectionId: "Statements", sectionLabel: "Statements", docs: [] },
      { sectionId: "Exhibits", sectionLabel: "Exhibits", docs: [] },
      { sectionId: "Forensics", sectionLabel: "Forensics", docs: [] },
      {
        sectionId: "Unused materials",
        sectionLabel: "Unused materials",
        docs: [],
      },
      { sectionId: "Defendant", sectionLabel: "Defendant", docs: [] },
      {
        sectionId: "Court preparation",
        sectionLabel: "Court preparation",
        docs: [],
      },
      { sectionId: "Communications", sectionLabel: "Communications", docs: [] },
    ] as Section[]);
  });
});
