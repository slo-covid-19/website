const github = require('@actions/github')
const core = require('@actions/core')

const ghToken = process.env['INPUT_TOKEN']

const context = github.context;
const gh = github.getOctokit(ghToken)

function createDeployment() {
    core.info(JSON.stringify(context.payload, null, 1))
}

function main() {
    try {
        var args = process.argv.slice(2);
        switch (args[0]) {
        case 'createDeployment':
            createDeployment()
            break;
        default:
            core.setFailed("Unknown action")
        }
    } catch(ex) {
        core.setFailed(ex)
    }
}

main()
