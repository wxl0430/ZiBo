# 获取最新的 Git tag
$latestTag = git describe --tags --abbrev=0

# 生成 changelog 并存储为最新 tag 名称的 Markdown 文件
git cliff -t ${latestTag} --output "..\CHANGELOG.md"
