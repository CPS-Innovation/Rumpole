import { Checkbox, FormControlLabel, FormGroup } from "@mui/material";
import { FC } from "react";
import { Spacer } from "../../../../presentation/common/Spacer";
import { FilterDetails, SearchState } from "../../hooks/useSearchState";
import { Header } from "./Header";

type FilterProps = {
  title: string;
  filterDetails: FilterDetails;
  setFilterParam: SearchState["setFilterParam"];
};

export const FilterMany: FC<FilterProps> = ({
  title,
  filterDetails: { name, items },
  setFilterParam,
}) => {
  const handleChange = (value: string, isSelected: boolean) =>
    setFilterParam(name, value, isSelected);

  return (
    <>
      <Header>{title}</Header>
      <Spacer sx={{ height: 10 }} />

      <FormGroup>
        {Object.entries(items)
          .sort((a, b) => (a[1].name > b[1].name ? 1 : -1))
          .map(([key]) => {
            const item = items[key];
            const label = `${item.name} (${item.count})`;
            const isChecked = item.isSelected;
            return (
              <FormControlLabel
                key={key}
                control={
                  <Checkbox
                    checked={isChecked}
                    onChange={(event) =>
                      handleChange(key, event.target.checked)
                    }
                  />
                }
                label={label}
              />
            );
          })}
      </FormGroup>

      <Spacer sx={{ height: 20 }} />
    </>
  );
};
