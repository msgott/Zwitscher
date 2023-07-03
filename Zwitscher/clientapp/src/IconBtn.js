//icon button component for comments and the comment section

function IconBtn({ Icon, isActive, color, children, ...props }) {
  return (
    <button
      className={`btn icon-btn ${isActive ? "icon-btn-active" : ""} ${
        color || ""
      }`}
      {...props}
    >
      {/*change classes depending if button is active or not, define color default empty */}

      {/*space out between icon and number, ives a class if children not null */}
      <span className={`${children != null ? "mr-1" : ""}`}>
        <Icon />
      </span>
      {children}
    </button>
  );
}

export default IconBtn;
