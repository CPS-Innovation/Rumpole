import { AsyncResult } from "../../../../common/types/AsyncResult";
import { CaseDocument } from "../../domain/CaseDocument";
import { MappedCaseDocument } from "../../domain/MappedCaseDocument";
import { getCategory } from "./document-category-definitions";

export const mapDocumentsState = (
  result: AsyncResult<CaseDocument[]>
): AsyncResult<MappedCaseDocument[]> =>
  result.status === "loading"
    ? result
    : {
        ...result,
        data: result.data.map((item, index) => ({
          ...item,
          tabSafeId: `d${index}`,
          category: getCategory(item),
        })),
      };
