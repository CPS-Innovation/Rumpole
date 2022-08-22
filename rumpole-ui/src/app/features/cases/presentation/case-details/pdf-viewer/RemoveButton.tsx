import classes from "./RemoveButton.module.scss";

export const RemoveButton = ({ onClick }: { onClick: () => void }) => (
  <div className="Tip">
    <div className={classes.button} onClick={onClick}>
      Remove redaction
    </div>
  </div>
);
