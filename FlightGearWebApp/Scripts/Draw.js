function DrawPlane(ctx, x, y) {
    ctx.fillStyle = "red";
    ctx.arc(x, y, 5, 0, 2 * Math.PI);
    ctx.fill();
    ctx.stroke();
}
function DrawMovment(ctx, prevX, prevY,curX,curY) {
    ctx.beginPath();
    ctx.moveTo(prevX, prevY);
    ctx.lineTo(curX, curY);
    ctx.stroke();
}