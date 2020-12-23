/**
 * @param {Response} response
 * @param {string} challenge
 * @returns {boolean}
 */
async function gatherResponse(response, challenge) {
  const { headers } = response
  const contentType = headers.get("content-type") || ""
  if (contentType.includes("application/json")) {
    const json = JSON.stringify(await response.json())
    return json["challenge"] == challenge
  }

  return false;
}

/**
 * @param {Request} request
 */
async function handleRequest(request) {
  const init = {
    headers: {
      "content-type": "application/json;charset=UTF-8",
    },
  }

  const body = await request.text()
  const params = JSON.parse(body)
  const hostname = params["hostname"].trim()
  const challenge = params["challenge"].trim()

  const url = new URL("https://example.com/");
  url.protocol = "http"
  url.hostname = hostname
  url.pathname = "/api/connectivity_check"

  const response = await fetch(url, init)
  const successful = await gatherResponse(response, challenge)

  if (successful) {
    return new Response(JSON.stringify({ "success": true }), init)
  }

  return new Response(JSON.stringify({ "success": false }), init)
}

addEventListener("fetch", event => {
  return event.respondWith(handleRequest(event.request))
})