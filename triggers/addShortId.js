// main trigger function
function addShortId() {
  var collection = getContext().getCollection();
  var request = getContext().getRequest();
  var docToCreate = request.getBody();

  // Reject documents that do not have a name property by throwing an exception.
  if (!docToCreate.url) {
    throw new Error('Document must include a "url" property.');
  }

  docToCreate["id"] = shortid();

  // update the document that will be created
  request.setBody(docToCreate);
}

var shortid = function() {
  return 'xxxxxxxx'.replace(/x/g, function(c) {
    return (Math.random()*36|0).toString(36);
  });
}