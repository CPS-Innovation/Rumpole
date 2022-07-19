import classes from "./Modal.module.scss";

type Props = {
  isVisible: boolean | undefined;
  handleClose?: () => void;
};

export const Modal: React.FC<Props> = ({
  isVisible,
  children,
  handleClose,
}) => {
  // govuk styling seems to lead to the html element being the thing
  //  that scrolls rather than body.  We want to prevent the main content
  //  scrolling when we scroll the modal content.
  //  todo: not especially pure/functional!
  const htmlElement = document.getElementsByTagName("html")[0];
  if (isVisible) {
    htmlElement.classList.add(classes.stopHtmlScroll);
  } else {
    htmlElement.classList.remove(classes.stopHtmlScroll);
  }

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
        {handleClose && (
          <div className={classes.closeContainer}>
            <button
              data-testid="btn-modal-close"
              type="button"
              className={classes.close}
              aria-label="Close"
              onClick={handleClose}
            ></button>
          </div>
        )}
        <div className={classes.contentContainer}>{children}</div>
      </div>
    </div>
  );
};
