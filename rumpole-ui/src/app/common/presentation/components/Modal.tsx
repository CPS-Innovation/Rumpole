import classes from "./Modal.module.scss";

type Props = {
  isVisible: boolean | undefined;
  handleClose: () => void;
};

export const Modal: React.FC<Props> = ({
  isVisible,
  children,
  handleClose,
}) => {
  return !isVisible ? null : (
    <div
      data-testid="div-modal"
      className={classes.modal}
      onClick={handleClose}
    >
      <div
        className={classes.modalContent}
        onClick={(e) => e.stopPropagation()}
      >
        {children}
        <button
          data-testid="btn-modal-close"
          type="button"
          className={classes.close}
          aria-label="Close"
          onClick={handleClose}
        ></button>
      </div>
    </div>
  );
};
