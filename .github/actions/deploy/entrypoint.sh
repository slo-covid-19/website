#!/bin/sh -l

mkdir -p ~/.kube/
echo "${INPUT_KUBECONFIG}" > ~/.kube/config

helm list --namespace ${INPUT_NAMESPACE}

# helm upgrade ${INPUT_RELEASE_NAME} ./helm-chart \
#     --install \
#     --atomic \
#     --namespace=${INPUT_NAMESPACE} \
#     --set=image.tag=${IMAGE_TAG} \
#     --set=ingress.rule="${INPUT_INGRESS_RULE}"