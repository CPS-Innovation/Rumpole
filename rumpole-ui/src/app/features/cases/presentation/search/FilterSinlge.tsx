import {
  FormControl,
  FormControlLabel,
  Radio,
  RadioGroup,
} from "@mui/material";
import { FC } from "react";
import { Spacer } from "../../../../common/presentation/components/Spacer";
import { FilterDetails, SearchState } from "../../hooks/useSearchState";
import { Header } from "./Header";

type FilterSingleProps = {
  title: string;
  filterDetails: FilterDetails;
  setFilterParam: SearchState["setFilterSingleParam"];
};

export const FilterSingle: FC<FilterSingleProps> = ({
  title,
  filterDetails: { name, items },
  setFilterParam,
}) => {
  const handleChange = (value: string | undefined) =>
    setFilterParam(name, value);

  const isAnyFilterApplied = Object.values(items).some(
    (item) => item.isSelected
  );

  return (
    <>
      <Header>{title}</Header>
      <Spacer sx={{ height: 10 }} />
      <FormControl component="fieldset">
        <RadioGroup
          aria-label="gender"
          defaultValue="female"
          name="radio-buttons-group"
          onChange={(e) => handleChange(e.target.value || undefined)}
        >
          <FormControlLabel
            control={<Radio value={""} />}
            checked={!isAnyFilterApplied}
            label={"All"}
          />
          {Object.entries(items).map(([key]) => {
            const item = items[key];
            const label = `${item.name} (${item.count})`;
            const isChecked = item.isSelected;
            return (
              <FormControlLabel
                key={key}
                control={<Radio value={key} checked={isChecked} />}
                label={label}
              />
            );
          })}
        </RadioGroup>
      </FormControl>

      <Spacer sx={{ height: 20 }} />
    </>
  );
};
