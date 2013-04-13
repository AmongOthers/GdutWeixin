(function() {
  var template = Handlebars.template, templates = Handlebars.templates = Handlebars.templates || {};
templates['favoriate'] = template(function (Handlebars,depth0,helpers,partials,data) {
  this.compilerInfo = [2,'>= 1.0.0-rc.3'];
helpers = helpers || Handlebars.helpers; data = data || {};
  var buffer = "", stack1, functionType="function", escapeExpression=this.escapeExpression, self=this;

function program1(depth0,data) {
  
  var buffer = "", stack1;
  buffer += "\r\n	<li data-index=\"";
  if (stack1 = helpers.Index) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.Index; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + "\" data-key=\"";
  if (stack1 = helpers.LocalKey) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.LocalKey; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + "\" ";
  stack1 = helpers['if'].call(depth0, depth0.IsDone, {hash:{},inverse:self.noop,fn:self.program(2, program2, data),data:data});
  if(stack1 || stack1 === 0) { buffer += stack1; }
  buffer += ">\r\n		<div>\r\n			<h1><span class=\"favoriateIndex\">";
  if (stack1 = helpers.Index) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.Index; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + "</span></h1>\r\n			<p class=\"favoriateTitle\">";
  if (stack1 = helpers.Title) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.Title; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + "</p>\r\n			<p>";
  if (stack1 = helpers.Author) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.Author; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + " / ";
  if (stack1 = helpers.Publisher) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.Publisher; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + " / ";
  if (stack1 = helpers.PublishYear) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.PublishYear; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + "</p>\r\n			<a href=\"/Library/Details?url=";
  if (stack1 = helpers.Url) { stack1 = stack1.call(depth0, {hash:{},data:data}); }
  else { stack1 = depth0.Url; stack1 = typeof stack1 === functionType ? stack1.apply(depth0) : stack1; }
  buffer += escapeExpression(stack1)
    + "\" data-role=\"button\" data-inline=\"true\" data-mini=\"true\" data-theme=\"b\" data-rel=\"dialog\">详细信息</a>\r\n		</div>\r\n	</li>\r\n";
  return buffer;
  }
function program2(depth0,data) {
  
  
  return "class=\"done\"";
  }

  stack1 = helpers.each.call(depth0, depth0, {hash:{},inverse:self.noop,fn:self.program(1, program1, data),data:data});
  if(stack1 || stack1 === 0) { buffer += stack1; }
  buffer += "\r\n";
  return buffer;
  });
})();