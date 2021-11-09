import { Checkbox, FormControlLabel, FormGroup } from "@mui/material";
import { FC } from "react";
import { InlineButton } from "../../../../common/presentation/components/InlineButton";
import { Spacer } from "../../../../common/presentation/components/Spacer";
import { FilterDetails, SearchState } from "../../hooks/useSearchState";
import { Header } from "./Header";

type FilterMultipleProps = {
  title: string;
  filterDetails: FilterDetails;
  setFilterParam: SearchState["setFilterMultipleParam"];
  setFilterParamAll: SearchState["setFilterMultipleParamAll"];
};

export const FilterMultiple: FC<FilterMultipleProps> = ({
  title,
  filterDetails: { name, items },
  setFilterParam,
  setFilterParamAll,
}) => {
  const handleChange = (value: string, isSelected: boolean) =>
    setFilterParam(name, value, isSelected);

  const handleClick = () => setFilterParamAll(name);

  return (
    <>
      <Header>{title}</Header>
      <Spacer sx={{ height: 10 }} />
      <InlineButton onClick={handleClick}>All</InlineButton>
      <FormGroup>
        {items.map((item) => {
          return (
            <FormControlLabel
              key={item.id}
              control={
                <Checkbox
                  checked={item.isSelected}
                  onChange={(event) =>
                    handleChange(item.id, event.target.checked)
                  }
                />
              }
              label={item.name}
            />
          );
        })}
      </FormGroup>

      <Spacer sx={{ height: 20 }} />
    </>
  );
};
