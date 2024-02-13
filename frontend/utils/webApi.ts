import { hash } from "ohash";

type HTTPMethod =
  | "GET"
  | "HEAD"
  | "POST"
  | "PUT"
  | "DELETE"
  | "CONNECT"
  | "OPTIONS"
  | "TRACE"
  | "PATCH";

interface FetcherConfig {
  readonly method: HTTPMethod;
  readonly body?: object;
  readonly config?: RequestInit;
  readonly params?: object;
  readonly query?: object;
  readonly onSuccess?: (response: any) => void;
}

export const useWebApiFetch = function (request: string, opts: FetcherConfig) {
  const config = useRuntimeConfig();

  return useFetch(request, {
    baseURL: config.public.BASE_URL as string,
    onRequest({ request, options }) {
      // Set the request headers
    },
    onRequestError(context) {},
    onResponse({ request, response, options }) {},
    onResponseError({ request, response, options }) {
      // Global error message
    },
    credentials: "include",
    key: hash([
      "webapi-fetch",
      request,
      opts?.body,
      opts?.params,
      opts?.method,
      opts?.query,
    ]),
    ...opts,
  });
};
